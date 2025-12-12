CREATE USER 'productservice' @'%' IDENTIFIED BY 'Product@123';

GRANT ALL PRIVILEGES ON ProductServiceDB.* TO 'productservice' @'%';

FLUSH PRIVILEGES;