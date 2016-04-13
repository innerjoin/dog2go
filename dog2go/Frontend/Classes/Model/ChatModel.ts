export class Message implements IMessage {
    constructor() {
        this.User = new User();
    }
    public Msg: string;
    public User: User;
    public Group: string;

}
export class User implements IUser {
    public Identifier: string;

    public Nickname: string;
        public GroupName: string;
        public Cookie: string;
}
