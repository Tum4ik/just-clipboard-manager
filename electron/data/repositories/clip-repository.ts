import { AppDataSource } from "../app-data-source";
import { Clip } from "../entities/clip";

export const clipRepository = AppDataSource.getRepository(Clip);
