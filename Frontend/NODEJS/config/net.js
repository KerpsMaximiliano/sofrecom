//NET CONNECTIONS

var net = {}

net.protocol = 'http';
net.host = 'localhost';
net.port = '9696';
net.path = '/api/customer';
net.customerUrl = net.protocol + '://' + net.host + ':' + net.port + net.path;

module.exports = net;



