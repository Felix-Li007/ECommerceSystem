CREATE USER 'adminweb' @'%' IDENTIFIED WITH mysql_native_password BY 'Admin123';
GRANT ALL PRIVILEGES ON AdminWebDB.* TO 'adminweb' @'%';
FLUSH PRIVILEGES;