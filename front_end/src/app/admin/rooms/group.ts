import { Range } from './range';

export class Group {
  Id: number;
  Name: string;
  Selected: boolean;
  SortOrder: number;
  TypeId: number;
  Ranges: [Range]
}
