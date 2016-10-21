import { Range } from './range';

export class Group {
  Id: number;
  Name: string;
  Selected: number;
  SortOrder: number;
  TypeId: number;
  Ranges: [Range]
}
