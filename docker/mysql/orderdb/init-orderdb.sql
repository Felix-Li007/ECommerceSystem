CREATE USER 'orderservice' @'%' IDENTIFIED BY 'Order@123';

GRANT ALL PRIVILEGES ON OrderServiceDB.* TO 'orderservice' @'%';

FLUSH PRIVILEGES;