var baseSchema = {
    sql_id: {type: Number, required: false},
    definite: {type: Boolean, required: true},
    user: {type: String, required: false}
};

module.exports = baseSchema;