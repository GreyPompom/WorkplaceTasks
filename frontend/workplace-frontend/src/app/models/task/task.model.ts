export interface Task {
  id: string;
  title: string;
  description?: string;
  status: 'Pending' | 'InProgress' | 'Done';
  createdAt: string;
  updatedAt: string;
  createdById: string;
}
