export interface TaskCreateDto {
  title: string;
  description?: string;
  status?: 'Pending' | 'InProgress' | 'Done';
}
