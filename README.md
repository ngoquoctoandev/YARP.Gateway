# YARP.Gateway
YARP reverse proxy for ASP.NET Core application, made by ngoquoctoandev with 💜

## Hướng dẫn sử dụng
1. Build docker image từ Dockerfile (chú ý vị trí đường dẫn thực hiện lệnh docker build)
2. Tạo container từ image vừa build
3. Để YARP giao tiếp với backend, cần chú ý:
    + Nếu backend đang ở môi trường Development, cập nhật "Address": "http://host.docker.internal:5000/api/v1" (Chỉ hiệu lực trên Windows, macOS. Với Linux thì phải thay bằng địa chỉ IP máy local)
    + Nếu backend đã publish và chạy dưới dạng 1 container ➜ "Address": "http://<container_ip>:<container_port>"
Địa chỉ ở trên đang để giao thức http, nếu muốn sử dụng giao thức https thì backend phải có chứng chỉ SSL hợp lệ. Sử dụng chứng chỉ tự ký có thể gây ra lỗi.
