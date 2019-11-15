import { Injectable } from "@angular/core";
import { LocalStorageService } from "ngx-store";
import { defer, Observable, of } from "rxjs";
import { UAParser } from "ua-parser-js";
import { environment } from "../../../environments/environment";
import { Machine } from "../../models/machine.model";
import { ApplicationSettings } from "../../models/settings.model";
import { PersistedUser, toUser } from "../../models/user.model";
import { RequireAdministrator } from "../../shared/require-admin.decorator";
import { SettingsService } from "../settings.service";
import { LogStorageService } from "./storage/log-storage.service";
import { MachineStorageService } from "./storage/machine-storage.service";
import { UserStorageService } from "./storage/user-storage.service";
import { NgxLoggerLevel } from "ngx-logger";

@Injectable({ providedIn: "root" })
export class LocalSettingsService implements SettingsService {
    constructor(
        private localStorage: LocalStorageService,
        private machineStorageService: MachineStorageService,
        private userStorageService: UserStorageService,
        private logStorageService: LogStorageService
    ) {}

    createAppSettings(): ApplicationSettings {
        const settings: ApplicationSettings = {
            hideDisabledMachines: false,
            hideIdleMachines: false,
            sortByTimeRemaining: false,
            interval: 10000
        };

        this.localStorage.set("settings", settings);
        return settings;
    }

    getConfigurationBundle(): Observable<any> {

        const self = this;
        return defer(async function() {
            const users: PersistedUser[] = await self.userStorageService.getUsers();
            const machines: Machine[] = await self.machineStorageService.getMachines();
            const settings: ApplicationSettings = self.localStorage.get("settings") || self.createAppSettings;

            return {
                users: users.map(u => toUser(u)),
                settings: settings,
                machines: machines
            };
        });
    }

    getSettings(): Observable<ApplicationSettings> {
        return of(this.localStorage.get("settings") || this.createAppSettings());
    }

    @RequireAdministrator()
    updateSettings(settings: ApplicationSettings): Observable<ApplicationSettings> {
        this.localStorage.set("settings", settings);
        return of(settings);
    }

    @RequireAdministrator()
    addCertificateException(certificateDetails: any): Observable<any> {
        // This isn't supported for the client side app as it isn't needed.
        // as long as the browser can open the page the app will be able to access
        // the api.
        return of(null);
    }

    getApplicationInfo(): Observable<any> {
        const parser = new UAParser();

        return of({
            platform: parser.getEngine().name,
            operatingSystem: parser.getOS().name,
            machineName: parser.getBrowser().name,
            version: environment.appVersion
        });
    }

    getLog(): Observable<string> {
        const self = this;
        return defer(async () => {
            const logEntries = await self.logStorageService.read();
            return logEntries
                .map(e => {
                    let message: any = e.message;
                    message = typeof message === "string" ? message : message.stack;
                    return `${e.timestamp} - ${NgxLoggerLevel[e.level]} in ${e.fileName}(${e.lineNumber}): ${message}`;
                })
                .join("\n");
        });
    }
}
