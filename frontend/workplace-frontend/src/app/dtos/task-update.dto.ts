export interface TaskUpdateDto {
  title: string;
  description?: string;
  status: 'Pending' | 'InProgress' | 'Done';
}
