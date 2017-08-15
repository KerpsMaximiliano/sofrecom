var mongoose = require('mongoose');
var Schema = mongoose.Schema;
var projectModel = require('./projectModel');

var projectSchema = new Schema(
    projectModel
);

module.exports = mongoose.model('Project', projectSchema);
