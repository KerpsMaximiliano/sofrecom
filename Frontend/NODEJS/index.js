var express = require('express');
var app = express();
var mongoose = require('mongoose');
var bodyParser = require('body-parser');
var methodOverride = require('method-override');
var http = require('http');
//customer
var customerModel = require('./models/customer');
var customerController = require('./controllers/customerController');
//brand
var brandModel = require('./models/brand');
var brandController = require('./controllers/brandController');
//product family 1
var productFamily1Model = require('./models/productFamily1');
var productFamily1Controller = require('./controllers/productFamily1Controller');
//product family 2
var productFamily2Model = require('./models/productFamily2');
var productFamily2Controller = require('./controllers/productFamily2Controller');
//product type
var productTypeModel = require('./models/productType');
var productTypeController = require('./controllers/productTypeController');

var io = require('socket.io');


//body parser
app.use(bodyParser.urlencoded({
    extended: false
}));
app.use(bodyParser.json());

//method override
app.use(methodOverride());

//root route
var rootRouter = express.Router();
rootRouter.route('/')
    .get(function(req, res){
        res.send("Hello Sofco!!");
    });

//customer router
var customerRouter = express.Router();
customerRouter.route('/customer')
    .get(customerController.getCustomers)
    .post(customerController.addCustomer);

//brand router
var brandRouter = express.Router();
brandRouter.route('/brand/:name?:_id?')
    .get(brandController.get)
    .post(brandController.add)
    .put(brandController.edit)
    .delete(brandController.delete);

//product family 1
var productFamily1Router = express.Router();
productFamily1Router.route('/productFamily1')
    .get(productFamily1Controller.getProductFamilies1)
    .post(productFamily1Controller.addProductFamily1)
    .put(productFamily1Controller.editProductFamily1)
    .delete(productFamily1Controller.deleteProductFamily1);

//product family 2
var productFamily2Router = express.Router();
productFamily2Router.route('/productFamily2')
    .get(productFamily2Controller.getProductFamilies2)
    .post(productFamily2Controller.addProductFamily2);

//product type
var productTypeRouter = express.Router();
productTypeRouter.route('/productType')
    .get(productTypeController.getProductTypes)
    .post(productTypeController.addProductType);

//Access-Control-Allow-Origin
app.use(function(req, res, next){
    res.setHeader('Access-Control-Allow-Origin', 'http://localhost:4200');
    next();
})

//mongoose connection
mongoose.connect('mongodb://localhost/sofco', function(err, res){
    if(err){
        console.log(err.message);
    } else{

        /*
        var server = http.createServer(app).listen(3000, function(){
          console.log("Express server listening on port " + app.get('port'));
        });
        var io = require('socket.io').listen(server);
        */
         
        var server = require('http').createServer(app);
        var io = require('socket.io')(server);

        io.on('connection', function (client) {
            console.log('Client connected...');

            client.on('join', function (data) {
                console.log(data);
            });
        });

        server.listen(3000, function(){
            //var io = require('socket.io').listen(app.server);
            console.log("Backend Sofco working on port 3000 ...");
        })
    }
});

app.use(rootRouter);
app.use(customerRouter);
app.use(brandRouter);
app.use(productFamily1Router);
app.use(productFamily2Router);
app.use(productTypeRouter);

app.use(function(req, res){
    res.type('text/html');
    res.status(404, 'Page not Found');
});

/*app.use(function(err, req, res, next){
    res.status(500).send({"Error": err.message});
    next();
});*/