export class ChatMessage {
    user: string;
    msg: string;
    time: string;

    constructor(p_user: string, p_msg: string, p_time: string) {
        this.user = p_user;
        this.msg = p_msg;
        this.time = p_time;
    }
};