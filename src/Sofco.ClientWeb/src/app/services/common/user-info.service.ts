export class UserInfoService {
    public static setUserInfo(userInfo: any) {
        localStorage.setItem('userInfo', JSON.stringify(userInfo));
    }

    public static getUserInfo(): any {
        return JSON.parse(localStorage.getItem('userInfo'));
    }
}