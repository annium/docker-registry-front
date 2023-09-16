format:
	xs format -sc -ic

setup:
	xs remote restore -user $(user) -password $(pass)

update:
	xs update all -debug -sc -ic

clean:
	xs clean -sc -ic

build:
	dotnet build --nologo -v q

test:
	@echo "noop"

publish: publish-server publish-site

configure:
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
	openssl genrsa -out run/keys/key.pem 4096
	openssl req -new -x509 -key run/keys/key.pem -out run/keys/cert.pem -days 360

server:
	cd Server && dotnet watch run

site:
	cd Site && dotnet watch run

site-css:
	cd Site && npx tailwindcss -i app.css -o wwwroot/app.css --watch

publish-server: build-server
	docker push annium/docker-registry-server

publish-site: build-site
	docker push annium/docker-registry-site

build-server:
	docker build -t annium/docker-registry-server -f Server/app.dockerfile Server

build-site:
	docker build -t annium/docker-registry-site -f Site/app.dockerfile Site

.PHONY: $(MAKECMDGOALS)