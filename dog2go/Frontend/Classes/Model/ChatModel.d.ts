interface IMessage {
    Msg: string;
    User: IUser;
    Group: string;

}
interface IUser {
    Identifier: string;
    Nickname: string;
    GroupName: string;
    Cookie: string;
}
