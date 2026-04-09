import type ErrorResponse from "$lib/models/ErrorResponse.ts";

class ErrorCustom extends Error {
    constructor(error: ErrorResponse) {
        super(error.message);

        // Set the prototype explicitly.
        Object.setPrototypeOf(this, ErrorCustom.prototype);
        this.error = error;
    }
    
    error: ErrorResponse;
}

export default ErrorCustom;