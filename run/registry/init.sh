#!/bin/sh

set -x

cp /certs/cert.crt /usr/local/share/ca-certificates
update-ca-certificates
registry serve /etc/docker/registry/config.yml
