var mongoose = require('mongoose');
var Schema = mongoose.Schema;
var serviceModel = require('./serviceModel');

var serviceSchema = new Schema(
    serviceModel
);

module.exports = mongoose.model('Service', serviceSchema);
