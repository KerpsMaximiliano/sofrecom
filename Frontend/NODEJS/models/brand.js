var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var brandSchema = new Schema({
    name: {type: String, required: true, unique:true}
});

module.exports = mongoose.model('Brand', brandSchema);