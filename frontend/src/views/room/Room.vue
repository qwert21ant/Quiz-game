<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title>
          <div class="mb-1 d-flex align-center">
            <v-btn
              class="mr-3"
              icon="mdi-arrow-left"
              variant="text"
              @click="back"
            />
            <div>Настройки комнаты</div>
            <div v-if="roomState.id" class="ml-auto d-flex align-center">
              <div>ID: {{ roomState.id }}</div>
              <v-btn
                class="ml-3"
                icon="mdi-content-copy"
                size="x-small"
                variant="outlined"
                @click="copyID"
              />
            </div>
          </div>
          <v-divider thickness="2"/>
        </v-card-title>
        <v-card-text style="padding-bottom: 0;">
          <v-text-field
            v-model="roomConfig.name"
            label="Name"
          />
          <v-text-field
            v-model="roomConfig.description"
            label="Description"
          />
          <v-select
            v-model="roomConfig.quizName"
            label="Quiz"
            :items="quizNames"
          />
          <v-text-field
            v-model="roomConfig.maxParticipants"
            type="number"
            label="Max participants"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn
            v-if="!roomState.open"
            class="ml-auto"
            variant="flat"
            prepend-icon="mdi-door-open"
            text="Открыть"
            @click="openRoom"
          />
          <v-btn
            v-if="roomState.open"
            class="ml-auto"
            variant="flat"
            prepend-icon="mdi-content-save"
            text="Обновить"
            @click="updateConfig"
          />
          <v-btn
            v-if="roomState.open"
            variant="outlined"
            color="error"
            prepend-icon="mdi-close"
            text="Закрыть"
            @click="closeRoom"
          />
        </v-card-actions>
      </v-card>
    </v-container>
    <v-container
      v-if="roomState.open"
      max-width="1000px"
    >
      <v-card>
        <v-card-title>
          Участники
        </v-card-title>
        <v-card-text>
          <div v-if="!roomState.participants.length">
            Нет участников
          </div>
          <div v-else class="d-flex flex-wrap">
            <v-card
              v-for="participant in roomState.participants"
              class="ma-1"
              width="250"
            >
              <v-card-title class="d-flex justify-space-between align-center">
                <div style="overflow-x: hidden; text-overflow: ellipsis;">{{ participant }}</div>
                <v-btn
                  class="ml-2"
                  color="error"
                  icon="mdi-close"
                  size="small"
                  variant="outlined"
                  @click="kickParticipant(participant)"
                />
              </v-card-title>
            </v-card>
          </div>
        </v-card-text>
      </v-card>
    </v-container>
  </div>
</template>

<script lang="ts">
import RoomConfig from '@/models/RoomConfig';
import UserData from '@/models/UserData';
import RoomService from '@/services/RoomService';
import UserService from '@/services/UserService';
import { defineComponent } from 'vue';
import router from '@/router';
import RoomState from '@/models/RoomState';

export default defineComponent({
  name: "Room",
  data() {
    return {
      userService: new UserService(),
      roomService: new RoomService(),

      userData: null as UserData,
      roomConfig: null as RoomConfig,
      roomState: null as RoomState,

      isInitialized: false as boolean,

      stateRefreshTimer: null,
    };
  },
  computed: {
    quizNames() {
      return this.userData.quizes.map(q => q.name);
    },
  },
  methods: {
    back() {
      router.push({ path: "/dashboard" });
    },
    copyID() {
      navigator.clipboard.writeText(this.roomState.id);
    },
    async refreshState() {
      this.roomState = await this.roomService.getState();
    },
    async updateConfig() {
      this.roomConfig.maxParticipants = Number(this.roomConfig.maxParticipants);
      await this.roomService.updateConfig(this.roomConfig);
    },
    async openRoom() {
      await this.updateConfig();
      await this.roomService.openRoom();
      await this.refreshState();

      this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);
    },
    async closeRoom() {
      await this.roomService.closeRoom();
      await this.refreshState();

      clearInterval(this.stateRefreshTimer);
    },
    async kickParticipant(participant: string) {
      await this.roomService.kickParticipant({ participant });
    }
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
    this.roomConfig = await this.roomService.getConfig();
    this.roomState = await this.roomService.getState();

    if (this.roomState.open)
      this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);

    this.isInitialized = true;
  },
});
</script>