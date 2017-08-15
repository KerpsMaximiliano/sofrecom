var baseSchema = require('./base');
var lo = require('lodash');

var base = lo.cloneDeep(baseSchema);

base.customerId = {type: Number, required: true};

module.exports = base;
