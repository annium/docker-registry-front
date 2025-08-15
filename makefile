setup:
	$(call header)
	dotnet tool restore

format:
	$(call header)
	dotnet tool run csharpier format . --config-path $(shell pwd)/.editorconfig
	dotnet tool run xs format -sc -ic

format-full: format
	$(call header)
	dotnet format style
	dotnet format analyzers

ensure-no-changes:
	$(call header)
	@if [[ -n "$$(git status --porcelain)" ]]; then \
		echo "Changes detected:"; \
		git status; \
		git --no-pager diff --no-color --exit-code; \
	fi

update:
	$(call header)
	dotnet tool list --format json | jq -r '.data[] | "\(.packageId)"' | xargs -I% dotnet tool install %
	dotnet tool run xs update all -sc -ic

clean:
	$(call header)
	dotnet tool run xs clean -sc -ic
	find . -type f -name '*.nupkg' | xargs -I% rm %

build:
	$(call header)
	$(call get-package-version)
	dotnet build -c Release --nologo -v q -p:PackageVersion=$(packageVersion)

test:
	$(call header)
	@echo "noop"

publish: publish-server publish-site

configure:
	$(call header)
	# registry
	cp run/registry/config.example.yml run/registry/config.yml
	# server
	cp run/server/config.example.json run/server/config.json
	cp run/server/config.example.json Server/config.json
	cp run/keys/key.pem run/server/key.pem
	cp run/keys/key.pem Server/key.pem
	# site
	cp run/site/config.example.json run/site/config.json
	cp run/site/config.example.json Site/wwwroot/config.json

keys:
	$(call header)
	openssl genrsa -out run/keys/key.pem 4096
	openssl req -new -x509 -key run/keys/key.pem -out run/keys/cert.pem -days 360

server:
	$(call header)
	cd Server && dotnet watch run

site:
	$(call header)
	cd Site && dotnet watch run

site-css:
	$(call header)
	cd Site && npx tailwindcss -i app.css -o wwwroot/app.css --watch

publish-server: build-server
	$(call header)
	docker push annium/docker-registry-server

publish-site: build-site
	$(call header)
	docker push annium/docker-registry-site

build-server:
	$(call header)
	docker build -t annium/docker-registry-server -f Server/app.dockerfile .

build-site:
	$(call header)
	docker build -t annium/docker-registry-site -f Site/app.dockerfile .

# CI
ci-merge-request-short:
	$(call header)
	make setup
	make format
	make ensure-no-changes
	make clean
	make build

ci-merge-request-full:
	$(call header)
	make setup
	make format
	make ensure-no-changes
	make docs-lint
	make clean
	make build
	make test
	make docs-build

ci-release:
	$(call header)
	make setup
	make format
	make ensure-no-changes
	make ci-set-package-version
	make clean
	make build
	make docs-build
	make publish
	make ci-push-tag repository=$(repository) githubToken=$(githubToken)
	echo "Release complete"

ci-set-package-version:
	$(call header)
# 	git config user.name "it"
# 	git config user.email "it@annium.com"
	dotnet tool run versioning set-version -v $(shell cat version)

ci-push-tag:
	$(call header)
	$(call get-package-version)
# 	git remote set-url origin https://x-access-token:$(githubToken)@github.com/$(repository).git
	git push origin v$(packageVersion)


define header
	@echo "=== $@ ==="
endef

define get-package-version
	$(eval packageVersion := $(shell dotnet tool run versioning get-version -v $(shell cat version)))
endef


.PHONY: $(MAKECMDGOALS)
