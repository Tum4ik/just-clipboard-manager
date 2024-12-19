import { BeforeInsert, Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@Entity()
export class Clip {
  @PrimaryGeneratedColumn()
  id!: number;

  @Column()
  pluginId!: `${string}-${string}-${string}-${string}-${string}`;

  @Column({ type: 'blob' })
  data!: Buffer;

  @Column({ type: 'blob' })
  representationData!: Buffer;

  @Column({ nullable: true })
  searchLabel?: string;

  @Column()
  clippedAt!: Date;


  @BeforeInsert()
  protected setClippedAt() {
    const timezoneOffset = new Date().getTimezoneOffset() * 60 * 1000; // minutes * to seconds * to milliseconds
    this.clippedAt = new Date(Date.now() - timezoneOffset);
  }
}
