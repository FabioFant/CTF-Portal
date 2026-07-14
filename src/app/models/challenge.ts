export interface Challenge {
  id: number;
  title: string;
  category: string;
  points: number;
  solved: boolean;
  dateAdded?: Date; 
}
