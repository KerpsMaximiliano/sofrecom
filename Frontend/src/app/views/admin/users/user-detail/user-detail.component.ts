import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { User } from 'models/user';
import { UserService } from './../../../../services/user.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
})
export class UserDetailComponent implements OnInit {

    public user = {};

    constructor(private service: UserService, private activatedRoute: ActivatedRoute, private router: Router) { }

    ngOnInit() {
        this.activatedRoute.params.subscribe(params => {
            var id = params['id'];
 
            this.service.getDetail(id).subscribe(user => {
                this.user = user;
            });
        });
    }
}
