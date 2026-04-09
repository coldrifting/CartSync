class ErrorResponse {
	statusCode: number = 0;
	message: string = "";
	errors: Record<string, string | null> | null = null;
	
	static asString(errorResponse: ErrorResponse): string {
		let error: string = errorResponse.statusCode + " " + errorResponse.message + "\n";
		
		if (errorResponse.errors) {
			for (let key in errorResponse.errors) {
				error += key + "\n" + errorResponse.errors[key]
			}
		}
		return error;
	}
}

export default ErrorResponse;