export class DateOfBirth {
  month: number;
  day: number;
  year: number;

  constructor(month = 0, day = 0, year = 0) {
    this.month = month;
    this.day = day;
    this.year = year;
  }
}
