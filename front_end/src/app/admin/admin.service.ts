import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import '../rxjs-operators';
import { HttpClientService } from '../shared/services';
import { EventParticipants, Room, Child, Group, Contact, Household, State, Country, NewParent } from '../shared/models';

@Injectable()
export class AdminService {
  site: number;

  constructor(private http: HttpClientService) {}

  getRooms(eventId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms`;
    return this.http.get(url)
                    .map(res => (<any[]>res.json()).map(r => Room.fromJson(r)))
                    .catch(this.handleError);
  }

  getBumpingRooms(eventId: string, roomId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/bumping`;
    return this.http.get(url)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  updateBumpingRooms(eventId: string, roomId: string, rooms: Room[]) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/bumping`;
    return this.http.post(url, rooms)
                    .map(res => res.json())
                    .catch(this.handleError);
  }

  updateRoom(eventId: string, roomId: string, body: Room) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}`;
    return this.http.put(url, body)
                    .map(res => Room.fromJson(res.json()))
                    .catch(this.handleError);
  }

  getRoomGroups(eventId: string, roomId: string) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/groups`;
    return this.http.get(url)
                    .map(res => Room.fromJson(res.json()))
                    .catch(this.handleError);
  }

  updateRoomGroups(eventId: string, roomId: string, room: Room) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/rooms/${roomId}/groups`;
    return this.http.put(url, room)
                    .map(res => Room.fromJson(res.json()))
                    .catch(err => {
                      return Observable.throw(JSON.parse(err.json().errors[0]));
                    });
  }

  importEvent(destinationEventId: number, sourceEventId: number): Observable<Room[]> {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${destinationEventId}/import/${sourceEventId}`;
    return this.http.put(url, null, null)
                    .map(res => { (<any[]>res.json()).map(r => Room.fromJson(r)); })
                    .catch(this.handleError);
  }

  createNewFamily(parents: Array<NewParent>) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/family`;
    return this.http.post(url, parents).map(res => { return res; }).catch(this.handleError);
  }

  getChildrenForEvent(eventId: number, searchString?: string) {
    let url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/children`;
    if (searchString) {
      url = `${url}?search=${searchString}`;
    }
    return this.http.get(url)
                    .map(res => { return (<Child[]>res.json()).map(r => Child.fromJson(r)); })
                    .catch(this.handleError);
  }


  getChildrenByHousehold(householdId: number, eventId: number): Observable<EventParticipants> {
    let url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/signin/children/household/${householdId}`;
    return this.http.get(url)
                    .map(res => {
                      let eventParticipants = EventParticipants.fromJson(res.json());
                      if (eventParticipants.hasParticipants()) {
                        eventParticipants.Participants.forEach(p => p.Selected = true);
                      }
                      return eventParticipants;
                    })
                    .catch(this.handleError);
  }

  getUnassignedGroups(eventId: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/events/${eventId}/groups/unassigned`;
    return this.http.get(url)
                    .map(res => { return Group.fromJsons(res.json()); })
                    .catch(this.handleError);
  }

  reprint(participantEventId: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/signin/participant/${participantEventId}/print`;
    return this.http.post(url, {}).catch(this.handleError);
  }

  reverseSignin(eventId: number, roomId: number, eventParticipantId: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/signin/event/${eventId}/room/${roomId}/reverse/${eventParticipantId}`;
    return this.http.put(url, null).catch(this.handleError);
  }

  findFamilySigninAndPrint(eventParticipants, numberEventsAttending) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/signin/familyfinder`;
    eventParticipants.ServicesAttended = numberEventsAttending;
    return this.http.post(url, eventParticipants)
                    .map(res => {
                      return EventParticipants.fromJson(res.json());
                    })
                    .catch(this.handleError);
  }

  findFamilies(searchString: string): Observable<Array<Contact>> {
    const url = `${process.env.ECHECK_API_ENDPOINT}/findFamily?search=${searchString}`;
    return this.http.get(url)
                    .map(res => {
                      let contacts = (<any[]>res.json()).map(r => Contact.fromJson(r));

                      return contacts;
                    })
                    .catch(this.handleError);
  }

  addFamilyMembers(contacts: Array<Contact>, householdId: number) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/family/${householdId}/member`;
    return this.http.post(url, contacts)
                    .map(res => {})
                    .catch(this.handleError);
  }

  updateFamilyMember(contact: Contact) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/family/member/${contact.ContactId}`;
    return this.http.put(url, contact)
                    .map(res => {})
                    .catch(this.handleError);
  }

  getHouseholdInformation(householdId: number): Observable<Household> {
    const url = `${process.env.ECHECK_API_ENDPOINT}/getHouseholdByID/${householdId}`;
    return this.http.get(url)
                    .map((res) => Household.fromJson(res.json()))
                    .catch(this.handleError);
  }

  updateHousehold(household: Household) {
    const url = `${process.env.ECHECK_API_ENDPOINT}/updateFamily`;
    return this.http.put(url, household)
                    .map((res) => Household.fromJson(res.json()))
                    .catch(this.handleError);
  }

  getStates(): Observable<Array<State>> {
    const url = `${process.env.ECHECK_API_ENDPOINT}/getStates`;
    return this.http.get(url)
                    .map((res) => (<any[]>res.json()).map(s => State.fromJson(s)))
                    .catch(this.handleError);
  }

  getCountries(): Observable<Array<Country>> {
    const url = `${process.env.ECHECK_API_ENDPOINT}/getCountries`;
    return this.http.get(url)
                    .map((res) => (<any[]>res.json()).map(c => Country.fromJson(c)))
                    .catch(this.handleError);
  }

  private handleError (error: any) {
    return Observable.throw(error.json().error || error.json().errors[0] || 'Server error');
  }
}
