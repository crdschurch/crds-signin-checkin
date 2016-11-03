import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { SetupService } from './setup.service';
import { RootService } from '../shared/services';

@Component({
  selector: 'setup',
  templateUrl: 'setup.component.html',
  styleUrls: ['setup.component.scss'],
  providers: [ SetupService ]
})
export class SetupComponent implements OnInit {
  machineId: string;
  isError: boolean;

  constructor(private setupService: SetupService,
              private route: ActivatedRoute,
              private rootService: RootService) {}

  reset() {
    document.cookie.split(';').forEach(function(c) {
      document.cookie = c.replace(/^ +/, '').replace(/=.*/, '=;expires=' + new Date().toUTCString() + ';path=/');
    });
    this.machineId = undefined;
  }

  ngOnInit() {
    let machineGuid;
    this.route.params.forEach((params: Params) => {
      this.isError = params['error'];
      machineGuid = params['machine'];
    });

    machineGuid = machineGuid || this.setupService.getMachineIdConfigCookie();
    if ( machineGuid ) {
      this.machineId = machineGuid;
      this.setupService.setMachineIdConfigCookie(this.machineId);
    }
  }
}