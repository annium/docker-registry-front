configure:
	# run - front
	cp run/front/config.example.json run/front/config.json
	cp run/keys/key.pem run/front/
	# debug - front
	cp run/front/config.example.json Server/config.json
	cp run/keys/key.pem Server
	cp run/front/appsettings.example.json Site/wwwroot/appsettings.json
	# run - registry
	cp run/registry/config.example.yml run/registry/config.yml
	cp run/keys/cert.pem run/registry/

keys:
	openssl genrsa -out run/keys/key.pem 4096
	openssl req -new -x509 -key run/keys/key.pem -out run/keys/cert.pem -days 360

server:
	cd Server && dotnet watch run

site:
	cd Site && dotnet watch run

site-css:
	cd Site && pnpx tailwindcss -i app.css -o wwwroot/app.css --watch

.PHONY: $(MAKECMDGOALS)