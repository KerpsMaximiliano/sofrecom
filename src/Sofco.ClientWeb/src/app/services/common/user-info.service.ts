export class UserInfoService {
    public static setUserInfo(userInfo: any) {
        console.log('> UserInfoService.setUserInfo <');
        localStorage.setItem('userInfo', JSON.stringify(userInfo));
    }

    public static getUserInfo(): any {
        console.log('> UserInfoService.getUserInfo <');
        return JSON.parse(localStorage.getItem('userInfo'));
    }
}