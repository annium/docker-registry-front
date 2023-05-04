configure:
	cp run/front/config.example.json run/front/config.json
	cp run/keys/key.pem run/front/
	cp run/registry/config.example.yml run/registry/config.yml
	cp run/keys/cert.pem run/registry/

keys:
	openssl genrsa -out run/keys/key.pem 4096
	openssl req -new -x509 -key run/keys/key.pem -out run/keys/cert.pem -days 360

.PHONY: $(MAKECMDGOALS)