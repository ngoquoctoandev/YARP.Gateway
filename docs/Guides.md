## Lệnh build container image tại thư mục gốc của dự án
docker build --no-cache --pull -f ./docker/Dockerfile -t yarpgateway:latest .

## Lệnh build container image tại đường dẫn ./docker
docker build --no-cache --pull -t yarpgateway:latest .

## Lệnh tạo 1 container để chạy trong docker
docker run --name yarpgateway -p 443:7979 -p 80:8989 --network fullstackhero --restart always -d yarpgateway