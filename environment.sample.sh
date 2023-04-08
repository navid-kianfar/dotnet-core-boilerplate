#!/usr/bin/env bash
export PORTAINER_VOL=$(pwd)/docker-data/portainer
export RABBITMQ_VOL=$(pwd)/docker-data/rabbitmq
export POSTGRES_VOL=$(pwd)/docker-data/postgres
export MINIO_VOL=$(pwd)/docker-data/minio
export NGINX_VOL=$(pwd)/docker-data/nginx
export CERTBOT_VOL=$(pwd)/docker-data/certbot

# $(docker run --rm httpd:2.4-alpine htpasswd -nbB admin 'f7dT9hmP3gM7' | cut -d ":" -f 2)
# $2y$05$JvbTrH4ttme3Qre0eAibz.1n6PKU2Kcien77Myfd4Tn.EdMdNfRKq
export PORTAINER_PASSWORD='$2y$05$QJ4Ik8O/GFaBXEj8Wo4rje/yHawl1j41Ufzu3Sf0enuanS.sfijYa'
export RABBITMQ_USERNAME='admin'
export RABBITMQ_PASSWORD=''
export MINIO_ACCESS_KEY=''
export MINIO_SECRET_KEY=''
export MINIO_USERNAME='admin'
export MINIO_PASSWORD=''
export POSTGRES_USERNAME=''
export POSTGRES_PASSWORD=''

export APP_ENV='Development'
export APP_NAME='MyApplication'
export APP_IP='localhost'
export APP_PORT='5050'
export APP_BUILD='1.0.0'

export APP_AUTH_ISSUER=APP_NAME
export APP_AUTH_SECRET=''
export APP_AUTH_SALT=''

export APP_DB_SERVER='localhost'
export APP_DB_PORT='5432'
export APP_DB_USER=POSTGRES_USERNAME
export APP_DB_PASS=POSTGRES_PASSWORD

export APP_STORAGE_SERVER='localhost'
export APP_STORAGE_PORT='9000'
export APP_STORAGE_USER=MINIO_ACCESS_KEY
export APP_STORAGE_PASS=MINIO_SECRET_KEY
export APP_STORAGE_BUCKET=APP_NAME

export APP_QUEUE_SERVER='localhost'
export APP_QUEUE_PORT='5672'
export APP_QUEUE_USER=RABBITMQ_USERNAME
export APP_QUEUE_PASS=RABBITMQ_PASSWORD
export APP_QUEUE_PREFIX=APP_NAME
