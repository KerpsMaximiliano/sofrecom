var baseSchema = {
    id: {type: Number, required: false},
    description: {type: String, required: true},
    active: {type: Boolean, required: false},
    startDate: {type: Date, required: false},
    endDate: {type: Date, required: false}
};

module.exports = baseSchema;