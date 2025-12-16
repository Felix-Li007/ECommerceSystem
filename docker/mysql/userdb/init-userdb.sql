CREATE USER 'userservice' @'%' IDENTIFIED WITH mysql_native_password BY 'User123';
GRANT ALL PRIVILEGES ON UserServiceDB.* TO 'userservice' @'%';
FLUSH PRIVILEGES;