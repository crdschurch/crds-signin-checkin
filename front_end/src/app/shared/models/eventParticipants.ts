import { Child } from './child';
import { Event } from '../../admin/events/event';

export class EventParticipants {
  CurrentEvent: Event;
  Participants: Array<Child>;

  static fromJson(json: any): EventParticipants {
    if (!json) {
      return new EventParticipants();
    }
    console.log("json", json);

    let eventParticipants = new EventParticipants();
    eventParticipants.CurrentEvent = Event.fromJson(json.CurrentEvent);
    eventParticipants.Participants = [];
    for (let p of json.Participants) {
      console.log(p);
      eventParticipants.Participants.push(Child.fromJson(p));
    }
    return eventParticipants;
  }
}
