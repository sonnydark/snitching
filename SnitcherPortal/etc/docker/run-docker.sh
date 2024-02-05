#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p e6ae189f-1023-4d28-8e16-1bda222b9ca1 -t
    fi
    cd ../
fi

docker-compose up -d
