import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SetupService } from './setup.service';
import { MachineConfiguration } from './machine-configuration';
import { RootService } from '../shared/services';

@Component({
  selector: 'setup',
  templateUrl: 'setup.component.html',
  styleUrls: ['setup.component.scss'],
  providers: [ SetupService ]
})
export class SetupComponent implements OnInit {
  machineConfig: MachineConfiguration;

  constructor(private setupService: SetupService,
              private route: ActivatedRoute,
              private rootService: RootService) {}

  reset() {
    document.cookie.split(';').forEach(function(c) {
      document.cookie = c.replace(/^ +/, '').replace(/=.*/, '=;expires=' + new Date().toUTCString() + ';path=/');
    });
  }

  ngOnInit() {
    const machineGuid = this.route.snapshot.queryParams['machine'];
    if (machineGuid) {
      this.setupService.getMachineConfiguration(machineGuid).subscribe(
        machineConfig => { console.log(machineConfig);this.machineConfig = machineConfig; },
        error => {console.error(error)}
      );
    }
  }
}
