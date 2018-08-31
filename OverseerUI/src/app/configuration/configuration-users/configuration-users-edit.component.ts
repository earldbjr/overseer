import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute, ParamMap } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { LocalStorage } from "ngx-store";
import { Observable } from "rxjs";

import { sessionLifetimes } from "../display-option.type";
import { ConfigurationService } from "../../shared/configuration.service";
import { AuthenticationService } from "../../shared/authentication.service";
import { DialogService } from "../../dialogs/dialog.service";
import { matchValidator } from "../../shared/validators";

@Component({
    templateUrl: "./configuration-users-edit.component.html",
    styleUrls: [
        "../configuration.scss",
        "./configuration-users-edit.component.scss"
    ]
})
export class ConfigurationUsersEditComponent implements OnInit {
    lifetimes = sessionLifetimes;
    form: FormGroup;

    @LocalStorage() activeUser: any;
    user: any;
    users: Array<any>;
    settings: any;

    constructor(
        private configurationService: ConfigurationService,
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private formBuilder: FormBuilder,
        private dialog: DialogService
    ) {
        this.form = this.formBuilder.group({
            id: [],
            password: [null, [Validators.min(8)]],
            confirmPassword: [],
            sessionLifetime: []
        }, {
            validator: matchValidator("password", "confirmPassword")
        });
    }

    ngOnInit() {
        this.route.paramMap
            .subscribe((params: ParamMap) => {
                this.configurationService.getSettingsBundle().subscribe(bundle => {
                    this.settings = bundle.settings;
                    this.users = bundle.users;

                    const userId = parseInt(params.get("id"), 10);
                    this.user = Object.assign({}, this.users.find(u => u.id === userId));

                    this.form.patchValue(this.user);
                });
            })
            .unsubscribe();
    }

    signOut() {
        if (this.user.id === this.activeUser.id) {
            this.authenticationService.logout().subscribe(() => {
                this.router.navigate(["/login"]);
            });
        } else {
            this.authenticationService.logoutUser(this.user.id).subscribe(user => {
                this.user = user;
            });
        }
    }

    delete() {
        if (this.users.length === 1 && this.settings.requiresAuthentication) {
            this.dialog.alert({
                titleKey: "warning",
                messageKey: "requiresAuthenticationPrompt"
            });

            return;
        }

        this.dialog.prompt({ messageKey: "deleteUserPrompt" })
            .afterClosed()
            .subscribe(result => {
                if (result) {
                    this.handleNetworkAction(this.configurationService.deleteUser(this.user));
                }
            });
    }

    save() {
        this.handleNetworkAction(this.configurationService.updateUser(this.form.value));
    }

    private handleNetworkAction(observable: Observable<any>) {
        this.form.disable();
        observable.subscribe(
            () => this.router.navigate(["/configuration/users"]),
            () => this.form.enable()
        );
    }
}
