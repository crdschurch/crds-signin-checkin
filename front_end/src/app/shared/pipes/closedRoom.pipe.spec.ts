import { ClosedRoomPipe } from './closedRoom.pipe';
import { Room } from '../models';

describe ('ClosedRoomPipe', () => {

  // test class is defined here, because we care about the logic in the pipe, not
  // interaction with the room class
  class TestRoom {
    public AllowSignIn: boolean;
    public Capacity: number;
  }

  this._rooms = [];
  let closedTestRoom = new TestRoom();
  closedTestRoom.AllowSignIn = false;
  closedTestRoom.Capacity = 0;
  let openTestRoom = new TestRoom();
  openTestRoom.AllowSignIn = true;
  openTestRoom.Capacity = 5;

  this._rooms.push(closedTestRoom);
  this._rooms.push(openTestRoom);

  let pipe = new ClosedRoomPipe();

  it('returns all rooms when not true', () => {
    let result = pipe.transform(this._rooms, false);
    expect(result.length).toBe(2);
  });

  it('returns filtered rooms when true', () => {
    let result = pipe.transform(this._rooms, true);
    expect(result.length).toBe(1);
  });

});
