var mongoose = require('mongoose');
var Schema = mongoose.Schema;
var baseSchema = require('./base');
var serviceModel = require('./serviceModel');
var lo = require('lodash');

var base = lo.cloneDeep(baseSchema);


base.email = {type: String, required: false};
base.services = [serviceModel];

var customerSchema = new Schema(
    base
);

module.exports = mongoose.model('Customer', customerSchema);
