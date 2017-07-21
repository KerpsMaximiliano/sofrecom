var mongoose = require('mongoose');
var Schema = mongoose.Schema;
var baseSchema = require('./base');
var lo = require('lodash');

var base = lo.cloneDeep(baseSchema);

base.name = {type: String, required: true};

var customerSchema = new Schema(
    base
);

module.exports = mongoose.model('Customer', customerSchema);
