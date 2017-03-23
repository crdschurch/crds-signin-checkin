import * as moment from 'moment';

export class Timeframe {
  start: any;
  end: any;
  id: number;

  constructor(weekObject: any) {
    this.start = weekObject.start;
    this.end = weekObject.end;
    this.id = moment(this.start).unix();
  }
}
