import axiosConfig from "@/config/axios";
import { AxiosResponse } from "axios";

export interface RegisterRequest {
  firstname: string;
  lastname: string;
  email: string;
  password: string;
}

export interface RegisterResponse {
  message: string;
}

export interface LoginRequest {
  email: string;
  password: String;
}

export interface LoginResponse {
  status: string;
  data: ResponseData;
}

export interface ResponseData {
  jwtToken: string;
  user: User;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  bio: string;
  createdAt: string;
  updatedAt: string;
}

export interface UserResponse {
  data: User;
}

export interface UpdatedProfile {
  firstname: string;
  lastname: string;
  bio: string;
}

class AuthService {
  static register = async (
    requestBody: RegisterRequest
  ): Promise<AxiosResponse<RegisterResponse>> => {
    return await axiosConfig.post("auth/register", requestBody);
  };

  static login = async (
    requestBody: LoginRequest
  ): Promise<AxiosResponse<LoginResponse>> => {
    return await axiosConfig.post("auth/login", requestBody);
  };

  static getCurrentUser = async (): Promise<AxiosResponse<UserResponse>> => {
    return await axiosConfig.get("auth/");
  };
}

export default AuthService;
