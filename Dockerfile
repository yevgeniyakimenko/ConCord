# ==========================================
# Stage 1: Build the React/Vite Frontend
# ==========================================
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend

# Enable Corepack for Yarn v4
RUN corepack enable

# Install frontend dependencies
COPY ConCordFrontEnd/package.json ConCordFrontEnd/yarn.lock ConCordFrontEnd/.yarnrc.yml ./
RUN yarn install --immutable

# Copy source and build static assets into /app/frontend/dist
COPY ConCordFrontEnd/ ./
RUN yarn build

# ==========================================
# Stage 2: Build and Publish the .NET Backend
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS backend-build
WORKDIR /app/backend

# Download the model directly from Hugging Face
RUN curl -fsSL -o ./ToxicDetector.mlnet \
    https://huggingface.co/wi12ylfdk/concord-toxic-detector/resolve/main/toxic-detector-tiny.mlnet

# Restore .NET dependencies
COPY ConCord/ConCord.csproj ./
RUN dotnet restore

# Copy backend source
COPY ConCord/ ./

# Copy compiled frontend assets from Stage 1 into wwwroot
COPY --from=frontend-build /app/frontend/dist ./wwwroot

# Publish backend in Release mode
RUN dotnet publish -c Release -o /app/publish

# ==========================================
# Stage 3: Production Runtime
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

COPY --from=backend-build /app/publish ./

ENV PORT=8081
EXPOSE 8081

ENTRYPOINT ["dotnet", "ConCord.dll"]
