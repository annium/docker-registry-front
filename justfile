set shell := ["bash", "-cu"]
set positional-arguments

[private]
default:
    @just --list

# base

setup:
    @echo "=== $0 ==="
    dotnet tool restore

format:
    @echo "=== $0 ==="
    dotnet tool run csharpier format . --config-path $(pwd)/.editorconfig
    dotnet tool run xs format -sc -ic

format-full: format
    @echo "=== $0 ==="
    dotnet format style
    dotnet format analyzers

ensure-no-changes:
    #!/usr/bin/env bash
    set -e
    echo "=== ensure-no-changes ==="
    if [[ -n "$(git status --porcelain)" ]]; then
        echo "Changes detected:"
        git status
        git --no-pager diff --no-color --exit-code
    fi

update:
    @echo "=== $0 ==="
    dotnet tool list --format json | jq -r '.data[] | "\(.packageId)"' | xargs -I% dotnet tool install %
    dotnet tool run xs update all -sc -ic

clean:
    @echo "=== $0 ==="
    dotnet tool run xs clean -sc -ic
    find . -type f -name '*.nupkg' | xargs -I% rm %

build:
    #!/usr/bin/env bash
    set -e
    echo "=== build ==="
    packageVersion=$(dotnet tool run versioning get-version -v $(cat version))
    dotnet build -c Release --nologo -v q -p:PackageVersion=$packageVersion

test:
    @echo "=== $0 ==="
    @echo "noop"

publish: publish-server publish-site

configure:
    @echo "=== $0 ==="
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
    @echo "=== $0 ==="
    openssl genrsa -out run/keys/key.pem 4096
    openssl req -new -x509 -key run/keys/key.pem -out run/keys/cert.pem -days 360

server:
    @echo "=== $0 ==="
    cd Server && dotnet watch run

site:
    @echo "=== $0 ==="
    cd Site && dotnet watch run

site-css:
    @echo "=== $0 ==="
    cd Site && npx tailwindcss -i app.css -o wwwroot/app.css --watch

publish-server: build-server
    @echo "=== $0 ==="
    docker push annium/docker-registry-server

publish-site: build-site
    @echo "=== $0 ==="
    docker push annium/docker-registry-site

build-server:
    @echo "=== $0 ==="
    docker build -t annium/docker-registry-server -f Server/app.dockerfile .

build-site:
    @echo "=== $0 ==="
    docker build -t annium/docker-registry-site -f Site/app.dockerfile .

# ci

ci-merge-request-short:
    #!/usr/bin/env bash
    set -e
    echo "=== ci-merge-request-short ==="
    just setup
    just format
    just ensure-no-changes
    just clean
    just build

ci-merge-request-full:
    #!/usr/bin/env bash
    set -e
    echo "=== ci-merge-request-full ==="
    just setup
    just format
    just ensure-no-changes
    just docs-lint
    just clean
    just build
    just test
    just docs-build

ci-release repository githubToken:
    #!/usr/bin/env bash
    set -e
    echo "=== ci-release ==="
    just setup
    just format
    just ensure-no-changes
    just ci-set-package-version
    just clean
    just build
    just docs-build
    just publish
    just ci-push-tag "$1" "$2"
    echo "Release complete"

ci-set-package-version:
    @echo "=== $0 ==="
    dotnet tool run versioning set-version -v $(cat version)

ci-push-tag repository githubToken:
    #!/usr/bin/env bash
    set -e
    echo "=== ci-push-tag ==="
    packageVersion=$(dotnet tool run versioning get-version -v $(cat version))
    git push origin v$packageVersion
