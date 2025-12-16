CREATE USER 'orderservice' @'%' IDENTIFIED WITH mysql_native_password BY 'Order123';
GRANT ALL PRIVILEGES ON OrderServiceDB.* TO 'orderservice' @'%';
FLUSH PRIVILEGES;