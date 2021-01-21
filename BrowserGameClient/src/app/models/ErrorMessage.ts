export class ErrorMessage {
    type: string;
    title: string;
    message: string;

    constructor(p_type: string, p_title: string, p_message: string) {
        this.type = p_type;
        this.title = p_title;
        this.message = p_message;
    }
}