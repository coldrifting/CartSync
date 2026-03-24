class ErrorResponse {
	constructor() {
		this.statusCode = -1;
		this.message = "";
		this.errors = null;
	}
	
	statusCode: number;
	message: string;
	errors: Record<string, string | null> | null;
	
	getErrorMsg(): string {
		let error: string = this.statusCode + " " + this.message;
		
		if (this.errors) {
			for (let key in this.errors) {
				error += key + "\n" + this.errors[key]
			}
		}
		return error;
	}
}

export default ErrorResponse;