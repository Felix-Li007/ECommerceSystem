CREATE USER 'userservice'@'%' IDENTIFIED BY 'User@123';
GRANT ALL PRIVILEGES ON UserServiceDB.* TO 'userservice'@'%';
FLUSH PRIVILEGES;