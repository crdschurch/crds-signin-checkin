import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { ApiService, SetupService, ChannelService, ChannelConfig, SignalrWindow } from '../shared/services';
import { ChildCheckinComponent } from './child-checkin.component';
import { RoomComponent } from './room';
import { RoomChildComponent } from './room/child';
import { childCheckinRouting } from './child-checkin.routes';

let channelConfig = new ChannelConfig();
channelConfig.url = `${process.env.SIGNALR_ENDPOINT}`;
channelConfig.hubName = 'EventHub';

@NgModule({
  declarations: [
    ChildCheckinComponent,
    RoomComponent,
    RoomChildComponent
  ],
  imports: [
    SharedModule,
    childCheckinRouting,
    MomentModule
  ],
  providers: [
    ApiService,
    SetupService,
    { provide: SignalrWindow, useValue: window },
    { provide: 'channel.config', useValue: channelConfig }
  ]
})

export class ChildCheckinModule { }
