configure:
	# registry
	cp run/registry/config.example.yml run/registry/config.yml
	# server
	cp run/server/config.example.json run/server/config.json
	cp run/server/config.example.json Server/config.json
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

.PHONY: $(MAKECMDGOALS)